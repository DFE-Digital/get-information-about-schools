require "cgi"
require "govuk_markdown"
require "pathname"

Nanoc::Filter.define(:govuk_markdown) do |content, params|
  mermaid_blocks = {}
  text_code_blocks = {}

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

  content = content.gsub(/^```text\s*\n(.*?)^```\s*$/m) do
    code_source = Regexp.last_match(1).rstrip
    placeholder = "TEXT_CODE_BLOCK_#{text_code_blocks.length}"
    text_code_blocks[placeholder] = %(
<pre class="app-code-block"><code>#{CGI.escapeHTML(code_source)}</code></pre>
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

  text_code_blocks.each do |placeholder, block|
    html = html.gsub(%r{<p class="govuk-body-m">\s*#{placeholder}(.*?)</p>}m) do
      trailing_content = Regexp.last_match(1).to_s.strip
      trailing_content.empty? ? block : "#{block}\n<p class=\"govuk-body-m\">#{trailing_content}</p>"
    end
    html = html.gsub(placeholder, block)
  end

  source_identifier = params.fetch(:source_identifier)
  source_path = Pathname.new(source_identifier.sub(%r{\A/}, ""))
  source_output = if source_path.to_s == "index.md"
                    Pathname.new("index.html")
                  elsif source_path.basename.to_s == "index.md"
                    source_path.dirname.join("index.html")
                  else
                    Pathname.new(source_path.to_s.sub(/\.md\z/, "")).join("index.html")
                  end

  html = html.gsub(/href="([^":?#]+)([?#][^"]*)?"/) do
    href_path = Regexp.last_match(1)
    suffix = Regexp.last_match(2).to_s

    if href_path.start_with?("/") || href_path.start_with?("mailto:") || href_path.start_with?("tel:")
      Regexp.last_match(0)
    else
      target_source = source_path.dirname.join(href_path).cleanpath
      target_output = if href_path.end_with?(".md")
                        if target_source.basename.to_s == "index.md"
                          target_source.dirname.join("index.html")
                        else
                          Pathname.new(target_source.to_s.sub(/\.md\z/, "")).join("index.html")
                        end
                      elsif href_path.end_with?("/")
                        target_source.join("index.html")
                      else
                        target_source
                      end

      relative_path = target_output.relative_path_from(source_output.dirname).to_s.tr("\\", "/")
      %(href="#{relative_path}#{suffix}")
    end
  end

  html
end
