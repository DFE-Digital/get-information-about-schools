require "cgi"
require "govuk_markdown"

Nanoc::Filter.define(:govuk_markdown) do |content, _params|
  mermaid_blocks = {}

  content = content.gsub(/^```mermaid\s*\n(.*?)^```\s*$/m) do
    mermaid_source = Regexp.last_match(1).rstrip
    placeholder = "MERMAID_BLOCK_#{mermaid_blocks.length}"
    mermaid_blocks[placeholder] = %(
<div class="mermaid">
#{CGI.escapeHTML(mermaid_source)}
</div>
)
    placeholder
  end

  html = GovukMarkdown.render(content, { headings_start_with: "l" })

  mermaid_blocks.each do |placeholder, block|
    html = html.gsub(%r{<p class="govuk-body-m">\s*#{placeholder}(.*?)</p>}m) do
      trailing_content = Regexp.last_match(1).to_s.strip
      trailing_content.empty? ? block : "#{block}\n<p class=\"govuk-body-m\">#{trailing_content}</p>"
    end
    html = html.gsub(placeholder, block)
  end

  html = html.gsub(/href="((?:\.\.?\/)[^":?#]+)\.md([?#][^"]*)?"/) do
    path = Regexp.last_match(1)
    suffix = Regexp.last_match(2).to_s
    output_path = path.end_with?("/index") || path == "./index" || path == "../index" ? "#{path}.html" : "#{path}/index.html"

    %(href="#{output_path}#{suffix}")
  end

  html = html.gsub(/href="((?:\.\.?\/)[^":?#]*\/)"/, 'href="\1index.html"')

  html
end
