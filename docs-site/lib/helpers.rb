use_helper Nanoc::Helpers::Rendering
require "uri"

def home_page?
  [item.identifier.to_s, item.identifier.without_ext.to_s, item.path].include?("/") ||
    [item.identifier.to_s, item.identifier.without_ext.to_s, item.path].include?("/index.html")
end

def home_href
  return "./index.html" if home_page?

  depth = item.identifier.without_ext.to_s.split("/").reject(&:empty?).length
  ("../" * depth) + "index.html"
end

def site_url
  @config[:site_url]
end

def base_path
  @config[:base_path]
end

def canonical_url(doc_item = item)
  return nil unless site_url

  URI.join(site_url.end_with?("/") ? site_url : "#{site_url}/", doc_item.path.sub(%r{\A/}, "")).to_s
end

def site_index_html
  tree = {}

  items
    .select { |doc_item| documentation_page?(doc_item) }
    .sort_by { |doc_item| doc_item.identifier.without_ext.to_s }
    .each do |doc_item|
      parts = doc_item.identifier.without_ext.to_s.split("/").reject(&:empty?)
      node = tree

      parts.each do |part|
        node[part] ||= { "_children" => {}, "_item" => nil }
        node = node[part]["_children"]
      end

      parent_key = parts.last
      parent = tree
      parts[0..-2].each do |part|
        parent = parent[part]["_children"]
      end
      parent[parent_key]["_item"] = doc_item
    end

  <<~HTML
    <section class="govuk-!-margin-top-8">
      <h2 class="govuk-heading-m">Site index</h2>
      <p class="govuk-body-m">All documentation pages in this site.</p>
      #{render_site_index_nodes(tree)}
    </section>
  HTML
end

def documentation_page?(doc_item)
  identifier = doc_item.identifier.without_ext.to_s
  identifier.start_with?("/") && identifier != "/" && identifier != "/index"
end

def render_site_index_nodes(nodes)
  entries = nodes
    .sort_by { |key, node| node_sort_key(key, node) }
    .map { |key, node| render_site_index_node(key, node) }
    .join

  %(<ul class="govuk-list govuk-list--bullet">#{entries}</ul>)
end

def render_site_index_node(key, node)
  label = node["_item"] ? page_title(node["_item"]) : section_title(key)
  content = node["_item"] ? %(<a href="#{relative_output_href(node["_item"])}" class="govuk-link">#{label}</a>) : label

  if node["_children"].any?
    %(<li>#{content}#{render_site_index_nodes(node["_children"])}</li>)
  else
    %(<li>#{content}</li>)
  end
end

def node_sort_key(key, node)
  item_identifier = node["_item"]&.identifier&.without_ext&.to_s
  explicit_order = {
    "/service/overview" => 0,
    "/service/container" => 1,
    "/service/back-end-component/component" => 2,
    "/service/front-end-component/component" => 3
  }
  explicit_key_order = {
    "overview" => 0,
    "container" => 1,
    "back-end-component" => 2,
    "front-end-component" => 3
  }

  [
    explicit_order.fetch(item_identifier, explicit_key_order.fetch(key, 99)),
    node["_item"] ? page_title(node["_item"]).downcase : section_title(key).downcase
  ]
end

def page_title(doc_item)
  doc_item[:title] || first_heading(doc_item) || section_title(doc_item.identifier.without_ext.to_s.split("/").last)
end

def first_heading(doc_item)
  doc_item.raw_content[/^\s*#\s+(.+)$/, 1]
end

def section_title(slug)
  slug.split("-").map(&:capitalize).join(" ")
end

def relative_output_href(target_item)
  segments = target_item.identifier.without_ext.to_s.split("/").reject(&:empty?)
  "./#{segments.join('/')}/index.html"
end
